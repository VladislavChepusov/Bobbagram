using Api.Models.Subscriptions;
using AutoMapper;
using DAL.Entities;
using DAL;
using System.Runtime.InteropServices;
using Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class SubscriptionService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public SubscriptionService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task Subscribe(SubscriptionRequest newSubscription, Guid userId)
        {
            var user = await _context.Users.Include(it => it.Subscriptions)
                .FirstOrDefaultAsync(it => it.Id == userId);

            if (user == null)
                throw new UserNotFoundException();

            var sub = user.Subscriptions!.FirstOrDefault(it => it.SubUserId == newSubscription.SubUserId);

            if (newSubscription.SubUserId == userId)
                throw new Exception("you can't subscribe to yourself");

            if (sub != null)
                throw new Exception("You have already subscribed");

            var Subuser = await _context.Users.Include(it => it.Subscriptions)
               .FirstOrDefaultAsync(it => it.Id == newSubscription.SubUserId);

            if (Subuser == null)
                throw new UserNotFoundException();

            var subDb = _mapper.Map<Subscription>(newSubscription);
            subDb.UserId = userId;
            subDb.User = user;
            subDb.SubUser = Subuser;

            await _context.Subscriptions.AddAsync(subDb);
            await _context.SaveChangesAsync();
        }

        public async Task UnSubscribe(SubscriptionRequest newSubscription, Guid userId)
        {
            var sub = await _context.Subscriptions
                .FirstOrDefaultAsync(it => it.UserId == userId && it.SubUserId == newSubscription.SubUserId);
            
            if (sub == null)
                throw new Exception("You have already unsubscribed");

            _context.Subscriptions.Remove(sub);
            await _context.SaveChangesAsync();
        }

        // Удалят подписчиков и подписки
        public async Task DeleteMeSub(Guid userId)
        {
            var sub = await _context.Subscriptions
                   .Where(it => it.SubUserId == userId)
                   .ToListAsync();
            if (sub != null)
                _context.Subscriptions.RemoveRange(sub);
            var subs1 = await _context.Subscriptions
                   .Where(it => it.UserId == userId)
                   .ToListAsync();

            if (subs1 != null)
                _context.Subscriptions.RemoveRange(subs1);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubscriptionModel>> GetSubscribers(Guid userId)
        {
            var Subscribers = await _context.Subscriptions
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Where(it => it.SubUserId == userId)
                .Select(x => _mapper.Map<SubscriptionModel>(x))
                .ToListAsync();

            if (Subscribers == null)
                throw new Exception("Subscribers not Found");

            return Subscribers;
        }

        public async Task<IEnumerable<SubscriptionModel>> GetSubscription(Guid userId)
        {
            var Subscription = await _context.Subscriptions
                .Include(x => x.SubUser).ThenInclude(x => x.Avatar)
                .Where(it => it.UserId == userId)
                .Select(x => _mapper.Map<SubscriptionModel>(x))
                .ToListAsync();
            if (Subscription == null)
                throw new Exception("Subscription not Found");

            return Subscription;
        }
    }
}

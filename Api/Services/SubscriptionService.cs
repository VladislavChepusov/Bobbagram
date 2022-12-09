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



        public async Task<IEnumerable<SubscriptionModel>> GetSubscription(Guid subUserId)
        {
            var subscriptions = await _context.Subscriptions
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Where(it => it.SubUserId == subUserId)
                .Select(x => _mapper.Map<SubscriptionModel>(x))
                .ToListAsync();

            if (subscriptions == null)
                throw new Exception("Subscription not Found");

            return subscriptions;
        }

        public async Task<IEnumerable<SubscriptionModel>> GetSubscribers(Guid userId)
        {
            var subscribers = await _context.Subscriptions
                .Include(x => x.SubUser).ThenInclude(x => x.Avatar)
                .Where(it => it.UserId == userId)
                .Select(x => _mapper.Map<SubscriptionModel>(x))
                .ToListAsync();
            if (subscribers == null)
                throw new Exception("Subscription not Found");

            return subscribers;
        }
    }
}

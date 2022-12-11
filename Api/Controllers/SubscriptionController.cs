using Api.Models.Subscriptions;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class SubscriptionController : ControllerBase
    {

        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task Subscribe([FromBody] SubscriptionRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _subscriptionService.Subscribe(request, userId);
            }
            else
                throw new Exception("you are not authorized");
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task UnSubscribe([FromBody] SubscriptionRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _subscriptionService.UnSubscribe(request, userId);
            }
            else
                throw new Exception("you are not authorized");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SubscriptionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<SubscriptionModel>> GetSubscription(Guid userId)
            => await _subscriptionService.GetSubscription(userId);


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SubscriptionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<SubscriptionModel>> GetSubscribers(Guid userId)
            => await _subscriptionService.GetSubscribers(userId);
    }
}

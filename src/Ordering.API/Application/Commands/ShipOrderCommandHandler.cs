﻿namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class ShipOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<ShipOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    /// <summary>
    /// Handler which processes the command when
    /// administrator executes ship order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetShippedStatus();
        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class ShipOrderIdentifiedCommandHandler(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<ShipOrderCommand, bool>> logger) : IdentifiedCommandHandler<ShipOrderCommand, bool>(mediator, requestManager, logger)
{
    protected override bool CreateResultForDuplicateRequest() => true; // Ignore duplicate requests for processing order.
}

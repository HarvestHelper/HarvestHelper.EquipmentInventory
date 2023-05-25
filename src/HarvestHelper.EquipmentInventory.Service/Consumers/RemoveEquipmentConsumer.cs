using HarvestHelper.Common;
using HarvestHelper.EquipmentInventory.Contracts;
using HarvestHelper.EquipmentInventory.Service.Entities;
using HarvestHelper.EquipmentInventory.Service.Exceptions;
using MassTransit;

namespace HarvestHelper.EquipmentInventory.Service.Consumers
{
    public class RemoveEquipmentConsumer : IConsumer<RemoveEquipment>
    {

        private readonly IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository;
        private readonly IRepository<EquipmentItem> equipmentItemsRepository;

        public RemoveEquipmentConsumer(IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository, IRepository<EquipmentItem> equipmentItemsRepository)
        {
            this.equipmentInventoryItemsRepository = equipmentInventoryItemsRepository;
            this.equipmentItemsRepository = equipmentItemsRepository;
        }

        public async Task Consume(ConsumeContext<RemoveEquipment> context)
        {

            var message = context.Message;

            var item = await equipmentItemsRepository.GetAsync(message.EquipmentItemId);

            if (item == null)
            {
                throw new UnknownEquipmentException(message.EquipmentItemId);
            }

            var equipmentInventoryItem = await equipmentInventoryItemsRepository.GetAsync(
                equipment => equipment.UserId == message.UserId && equipment.EquipmentItemId == message.EquipmentItemId);

            if (equipmentInventoryItem != null)
            {
                // update if needed
                await equipmentInventoryItemsRepository.RemoveAsync(equipmentInventoryItem.Id);
            }

            await context.Publish(new EquipmentGranted(message.CorrelationId));
        }
    }
}
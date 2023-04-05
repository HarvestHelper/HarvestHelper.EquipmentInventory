using HarvestHelper.Common;
using HarvestHelper.EquipmentInventory.Contracts;
using HarvestHelper.EquipmentInventory.Service.Entities;
using HarvestHelper.EquipmentInventory.Service.Exceptions;
using MassTransit;

namespace HarvestHelper.EquipmentInventory.Service.Consumers
{
    public class GrantEquipmentConsumer : IConsumer<GrantEquipment>
    {

        private readonly IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository;
        private readonly IRepository<EquipmentItem> equipmentItemsRepository;

        public GrantEquipmentConsumer(IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository, IRepository<EquipmentItem> equipmentItemsRepository)
        {
            this.equipmentInventoryItemsRepository = equipmentInventoryItemsRepository;
            this.equipmentItemsRepository = equipmentItemsRepository;
        }

        public async Task Consume(ConsumeContext<GrantEquipment> context)
        {

            var message = context.Message;

            var item = await equipmentItemsRepository.GetAsync(message.EquipmentItemId);

            if (item == null)
            {
                throw new UnknownEquipmentException(message.EquipmentItemId);
            }

            var equipmentInventoryItem = await equipmentInventoryItemsRepository.GetAsync(
                equipment => equipment.FarmId == message.FarmId && equipment.EquipmentItemId == message.EquipmentItemId);

            if (equipmentInventoryItem == null)
            {
                equipmentInventoryItem = new EquipmentInventoryItem
                {
                    EquipmentItemId = message.EquipmentItemId,
                    FarmId = message.FarmId,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await equipmentInventoryItemsRepository.CreateAsync(equipmentInventoryItem);
            }
            else
            {
                await equipmentInventoryItemsRepository.UpdateAsync(equipmentInventoryItem);
            }

            await context.Publish(new EquipmentGranted(message.CorrelationId));
        }
    }
}
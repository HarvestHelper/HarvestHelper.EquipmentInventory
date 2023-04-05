namespace HarvestHelper.EquipmentInventory.Contracts
{
    public record GrantEquipment(Guid FarmId, Guid EquipmentItemId, Guid CorrelationId);

    public record EquipmentGranted(Guid CorrelationId);

    public record RemoveEquipment(Guid FarmId, Guid EquipmentItemId, Guid CorrelationId);

    public record EquipmentRemoved(Guid CorrelationId);

}
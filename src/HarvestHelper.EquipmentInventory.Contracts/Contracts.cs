namespace HarvestHelper.EquipmentInventory.Contracts
{
    public record GrantEquipment(Guid UserId, Guid EquipmentItemId, Guid CorrelationId);

    public record EquipmentGranted(Guid CorrelationId);

    public record RemoveEquipment(Guid UserId, Guid EquipmentItemId, Guid CorrelationId);

    public record EquipmentRemoved(Guid CorrelationId);

}
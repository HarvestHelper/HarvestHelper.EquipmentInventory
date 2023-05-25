using System;

namespace HarvestHelper.EquipmentInventory.Service.Dtos
{
    public record GrantEquipmentDto(Guid UserId, Guid EquipmentItemId);

    public record EquipmentInventoryItemDto(Guid EquipmentItemId, string Name, DateTimeOffset AcquiredDate);

    public record EquipmentItemDto(Guid Id, string Name);
}
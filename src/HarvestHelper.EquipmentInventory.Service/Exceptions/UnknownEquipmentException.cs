using System.Runtime.Serialization;

namespace HarvestHelper.EquipmentInventory.Service.Exceptions
{
    [Serializable]
    internal class UnknownEquipmentException : Exception
    {

        public UnknownEquipmentException(Guid equipmentItemId) : base($"Unknown equipment '{equipmentItemId}'")
        {
            this.EquipmentId = equipmentItemId;
        }

        public Guid EquipmentId { get; }

    }
}
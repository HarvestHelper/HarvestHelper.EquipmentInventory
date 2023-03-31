using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HarvestHelper.Common;
using HarvestHelper.EquipmentInventory.Service;
using HarvestHelper.EquipmentInventory.Service.Dtos;
using HarvestHelper.EquipmentInventory.Service.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("equipmentInventory")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";

        private readonly IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository;
        private readonly IRepository<EquipmentItem> equipmentItemsRepository;

        public ItemsController(IRepository<EquipmentInventoryItem> equipmentInventoryItemsRepository, IRepository<EquipmentItem> equipmentItemsRepository)
        {
            this.equipmentInventoryItemsRepository = equipmentInventoryItemsRepository;
            this.equipmentItemsRepository = equipmentItemsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EquipmentInventoryItemDto>>> GetAsync(Guid farmId)
        {
            if (farmId == Guid.Empty)
            {
                return BadRequest();
            }

            var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (Guid.Parse(currentUserId) != farmId)
            {
                if(!User.IsInRole(AdminRole))
                {
                    return Forbid();
                }
            }

            var equipmentInventoryItemEntities = await equipmentInventoryItemsRepository.GetAllAsync(equipment => equipment.FarmId == farmId);
            var equipmentInventoryItemIds = equipmentInventoryItemEntities.Select(equipment => equipment.EquipmentItemId);
            var equipmentItemEntities = await equipmentItemsRepository.GetAllAsync(equipment => equipmentInventoryItemIds.Contains(equipment.Id));

            var equipmentInventoryItemDtos = equipmentInventoryItemEntities.Select(equipmentInventoryItem =>
            {
                var equipmentItem = equipmentItemEntities.Single(equipmentItem => equipmentItem.Id == equipmentInventoryItem.EquipmentItemId);
                return equipmentInventoryItem.AsDto(equipmentItem.Name);
            });

            return Ok(equipmentInventoryItemDtos);
        }

        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> PostAsync(GrantEquipmentDto grantEquipmentDto)
        {
            var equipmentInventoryItem = await equipmentInventoryItemsRepository.GetAsync(
                equipment => equipment.FarmId == grantEquipmentDto.FarmId && equipment.EquipmentItemId == grantEquipmentDto.EquipmentItemId);

            if (equipmentInventoryItem == null)
            {
                equipmentInventoryItem = new EquipmentInventoryItem
                {
                    EquipmentItemId = grantEquipmentDto.EquipmentItemId,
                    FarmId = grantEquipmentDto.FarmId,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await equipmentInventoryItemsRepository.CreateAsync(equipmentInventoryItem);
            }
            else
            {
                await equipmentInventoryItemsRepository.UpdateAsync(equipmentInventoryItem);
            }

            return Ok();
        }
    }
}
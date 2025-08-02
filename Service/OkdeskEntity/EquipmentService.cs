using System.Threading.Tasks;
using System.Collections.Generic;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.Core.Api;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;
namespace AqbaApp.Service.OkdeskEntity
{
    public class EquipmentService(SettingService<MainSettings> mainSettings, Immutable immutable, GetItemService request)
    {

        public async Task<List<Equipment>?> GetEquipmentsByMaintenanceEntityFromCloudApi(long maintenanceEntityId)
        {
            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/equipment/by_maintenance_entity?maintenanceEntityId={maintenanceEntityId}";
            return await request.GetRangeOfItems<Equipment>(link);
        }

        public async Task<List<Equipment>?> GetEquipmentsByCompanyFromCloudApi(long companyId)
        {
            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/equipment/by_company?companyId={companyId}";

            return await request.GetRangeOfItems<Equipment>(link);
        }

        public async Task<Equipment?> GetEquipment(long equipmentId)
        {
            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/equipment?id={equipmentId}";

            return await request.GetItem<Equipment>(link);
        }
    }
}

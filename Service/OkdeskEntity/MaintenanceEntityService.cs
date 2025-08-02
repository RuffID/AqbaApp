using System.Threading.Tasks;
using System.Collections.Generic;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.Model.Client;
using AqbaApp.Core.Api;
using AqbaApp.Service.Client;

namespace AqbaApp.Service.OkdeskEntity
{
    public class MaintenanceEntityService(SettingService<MainSettings> mainSettings, Immutable immutable, GetItemService request)
    {
        public async Task<List<MaintenanceEntity>?> GetMaintenanceEntitiesFromCloudApi(long startIndex = 0, long limit = 50)
        {
            List<MaintenanceEntity>? maintenanceEntities = [];

            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/maintenanceEntity/list";
            await foreach (List<MaintenanceEntity> maintenanceEntitiesFromCloudApi in request.GetAllItems<MaintenanceEntity>(link, startIndex, limit))
            {
                maintenanceEntities.AddRange(maintenanceEntitiesFromCloudApi);
            }

            return maintenanceEntities;
        }
    }
}

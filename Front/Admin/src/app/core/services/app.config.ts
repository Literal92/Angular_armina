import { InjectionToken } from "@angular/core";

export let APP_CONFIG = new InjectionToken<string>("app.config");

export interface IAppConfig {
  apiEndpointCommon: string;
  apiEndpointProvider: string;
  apiSettingsPath: string;
  apiAdminPathFront: string;
}

export const AppConfig: IAppConfig = {
  apiEndpointCommon: "https://localhost:44381/api/Common",
 apiEndpointProvider: "https://localhost:44381/api/Admin",
   // apiEndpointCommon: "https://localhost:5004/api/Common",
    //  apiEndpointProvider: "https://localhost:5004/api/Admin",
//  apiEndpointCommon: "https://niniharaji.com/api/Common",
// apiEndpointProvider: "https://niniharaji.com/api/Admin",
  apiSettingsPath: "ApiSettings",
  apiAdminPathFront: ""
};

import { InjectionToken } from '@angular/core';

export let APP_CONFIG = new InjectionToken<string>('app.config');

export interface IAppConfig {
  apiEndpointCommon: string;
  apiEndpointClient: string;
  apiEndpointFront: string;
  apiSettingsPath: string;
  apiAdminPathFront: string;
}

export const AppConfig: IAppConfig = {
  //apiEndpointCommon: 'https://localhost:44381/api/Common',
  //apiEndpointClient: 'https://localhost:44381/api/client',

  //apiEndpointCommon: 'https://localhost:5004/api/Common',
  //apiEndpointClient: 'https://localhost:5004/api/client',

   apiEndpointCommon: 'http://angular.arminatrading.ir/api/Common',
  apiEndpointClient: 'http://angular.arminatrading.ir/api/client',
  apiEndpointFront: '',
  apiSettingsPath: 'ApiSettings',
  apiAdminPathFront: ''
};

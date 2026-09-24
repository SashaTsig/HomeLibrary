import { environment } from '../../environments/environment';

export class BaseApiClient {
    protected getBaseUrl(baseUrl: string) : string {
        
        let url = environment.apiUrl;

        return url.replace(/\/$/, '');
    }

}
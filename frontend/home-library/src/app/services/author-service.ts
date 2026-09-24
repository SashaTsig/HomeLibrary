import { inject, Injectable } from '@angular/core';
import { AuthorDto, AuthorsClient } from '../apiClient';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {
    private apiClient = inject(AuthorsClient);

    getAll(): Observable<AuthorDto[]> {
            return this.apiClient.authors();
    }
}

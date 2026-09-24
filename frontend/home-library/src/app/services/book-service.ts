import { Injectable, inject } from '@angular/core';
import {BookDto, BooksClient} from '../apiClient';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookService {

    private apiClient = inject(BooksClient);

    findBooks(title: string | undefined, author: string | undefined, content: string | undefined, ): Observable<BookDto[]> {
        return this.apiClient.booksAll(title, content, author);
    }

    saveBook(book: BookDto): Observable<BookDto | number> {
        return book.id ? this.apiClient.booksPUT(book.id, book) : this.apiClient.booksPOST(book);
    }

    getById(id: number): Observable<BookDto> {
        return this.apiClient.booksGET(id);
    }
}

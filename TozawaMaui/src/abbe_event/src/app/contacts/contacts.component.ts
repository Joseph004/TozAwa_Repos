import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-contacts',
    standalone: true,
    imports: [TranslateModule],
    templateUrl: './contacts.component.html',
    styleUrl: './contacts.component.css',
})
export class ContactsComponent { }
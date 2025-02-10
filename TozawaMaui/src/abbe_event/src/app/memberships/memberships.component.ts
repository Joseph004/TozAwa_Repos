import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-memberships',
    standalone: true,
    imports: [TranslateModule],
    templateUrl: './memberships.component.html',
    styleUrl: './memberships.component.css',
})
export class MembershipsComponent { }
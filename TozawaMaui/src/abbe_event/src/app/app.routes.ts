
import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { EventsComponent } from './events/events.component';
import { AboutComponent } from './about/about.component';
import { MembershipsComponent } from './memberships/memberships.component';
import { ContactsComponent } from './contacts/contacts.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'events', component: EventsComponent },
    { path: 'about', component: AboutComponent },
    { path: 'memberships', component: MembershipsComponent },
    { path: 'contacts', component: ContactsComponent }
];

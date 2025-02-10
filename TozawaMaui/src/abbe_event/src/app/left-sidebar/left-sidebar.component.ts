import { CommonModule } from '@angular/common';
import { Component, input, OnInit, output, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LangChangeEvent, TranslateModule, TranslateService, TranslationChangeEvent } from '@ngx-translate/core';
import { SideBarRoutes } from '../Models/sideBarRoutes';

@Component({
    selector: 'app-left-sidebar',
    standalone: true,
    imports: [RouterModule, CommonModule, TranslateModule],
    templateUrl: './left-sidebar.component.html',
    styleUrl: './left-sidebar.component.css',
})
export class LeftSidebarComponent implements OnInit, AfterViewInit {
    isLeftSidebarCollapsed = input.required<boolean>();
    changeIsLeftSidebarCollapsed = output<boolean>();
    items: SideBarRoutes[] = [
        {
            routeLink: 'home',
            icon: 'fal fa-home',
            label: 'home'
        },
        {
            routeLink: 'events',
            icon: 'fal fa-calendar-alt',
            label: 'events'
        },
        {
            routeLink: 'memberships',
            icon: 'fal fa-user-alt',
            label: 'memberships'
        },
        {
            routeLink: 'about',
            icon: 'fal fa-info',
            label: 'about'
        },
        {
            routeLink: 'contacts',
            icon: 'fal fa-address-book',
            label: 'contacts'
        }
    ];

    constructor(private translate: TranslateService, private changeDetectorRef: ChangeDetectorRef) {

    }

    ngAfterViewInit(): void {
        this.changeDetectorRef.detectChanges();
    }
    ngOnInit(): void {
        // this language will be used as a fallback when a translation isn't found in the current language
        this.translate.setDefaultLang('en');

        this.translate.onTranslationChange.subscribe((event: TranslationChangeEvent) => {
           
        });
        this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
           
        });
    }

    getTranslate(key: string): string {
        var result = '';
        this.translate.get(key).subscribe((res: string) => {
            result = res;
        });
        return result;
    }

    toggleCollapse(): void {
        this.changeIsLeftSidebarCollapsed.emit(!this.isLeftSidebarCollapsed());
    }

    closeSidenav(): void {
        this.changeIsLeftSidebarCollapsed.emit(true);
    }
}
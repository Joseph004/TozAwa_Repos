import { CommonModule } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
    selector: 'app-main',
    standalone: true,
    imports: [RouterOutlet, CommonModule, HeaderComponent, FooterComponent],
    templateUrl: './main.component.html',
    styleUrl: './main.component.css',
})
export class MainComponent {
    isLeftSidebarCollapsed = input.required<boolean>();
    screenWidth = input.required<number>();
    sizeClass = computed(() => {
        const isLeftSidebarCollapsed = this.isLeftSidebarCollapsed();
        if (isLeftSidebarCollapsed) {
            return '';
        }
        return this.screenWidth() > 768 ? 'main-body-trimmed' : 'main-body-md-screen';
    });
}
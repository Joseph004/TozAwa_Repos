import { Directive, ElementRef, Output, EventEmitter, HostListener } from '@angular/core';

@Directive({
    selector: '[appClickOutside]',
    standalone: true
})
export class ClickOutsideDirective {

    @Output()
    public clickOutside = new EventEmitter<void>();

    constructor(private elementRef: ElementRef) { }

    @HostListener('document:click', ['$event']) onDocumentClick(event: MouseEvent): void {
        const targetElement = event.target as HTMLElement;

        // Check if clicked outside the element
        if (targetElement && !this.elementRef.nativeElement.contains(targetElement)) {
            this.clickOutside.emit();
        }
    }
}
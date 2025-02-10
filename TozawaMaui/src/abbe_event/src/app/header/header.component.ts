import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [TranslateModule, CommonModule, FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  title = 'angular-i18n';
  selectedLanguage = 'en';
  isOpen: boolean = false;

  constructor(private translateService: TranslateService) { }

  ngOnInit(): void {
    this.isOpen = false;
  }

  onLanguageChange() {
    this.translateService.use(this.selectedLanguage)
  }

  getClassForActiveFlag(culture: string) {
    return `flag-icon flag-icon-${culture}`
  }
  getClassForCurrentFlag() {
    return `flag-icon flag-icon-${this.selectedLanguage === 'en' ? 'us' : this.selectedLanguage}`
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  public onOutsideClick() {
    this.isOpen = false;
  }

  selectLanguage = (activeCulture: string): void => {
    if(activeCulture === this.selectedLanguage) return;
    this.selectedLanguage = activeCulture;
    this.onLanguageChange();
    this.isOpen = false;
  }

  openBasket() {
  }
}
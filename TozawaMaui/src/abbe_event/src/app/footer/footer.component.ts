import { Component } from '@angular/core';
@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {

    getText(){
        return "@2025 DRC Afro Connect. All Rights Reserved. | Website design by @Quatriple-Concept";
    }
}
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  isLocked: boolean = false; // Default state is unlocked

  lock() {
    this.isLocked = true;
  }

  unlock() {
    this.isLocked = false;
  }
}

import {Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from "@angular/common/http";
import {AsyncPipe, NgForOf} from "@angular/common";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AsyncPipe, NgForOf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title: string = 'My Blog';
  users$: any = this.http.get('http://localhost:5285/users');

  constructor(private http: HttpClient) {}

  ngOnInit(): void {}
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommonService } from 'src/app/shared/components/services/common.service';

@Injectable({ providedIn: 'root' })
export class NavBarService {
  constructor(
    private http: HttpClient,
    private commonService: CommonService
  ) {}
}

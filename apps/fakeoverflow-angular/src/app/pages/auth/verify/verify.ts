import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Optional} from '@utils/types';
import {Authentication} from '@services/authentication';
import {HotToastService} from '@ngxpert/hot-toast';
import {AuthService} from 'fakeoverflow-angular-services';

@Component({
  selector: 'app-verify',
  imports: [],
  templateUrl: './verify.html',
  styleUrl: './verify.scss'
})
export class Verify implements OnInit, OnDestroy{

  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(HotToastService);

  private tokenId : Optional<string> = undefined;
  protected verificationStatus : 'verifying' | 'failed' | 'success' = 'verifying';
  private timeoutId : Optional<number> = undefined;

   ngOnInit() {
    this.tokenId = this.activatedRoute.snapshot.paramMap.get('verify-id') ?? undefined;
    if(this.tokenId !== undefined){
      this.verify();
    }
   }

   public get isGeneralPage(){
     return this.tokenId === undefined;
   }

   verify() {
     this.authService.verify(this.tokenId!)
       .subscribe({
         next: (response) => {
           this.verificationStatus = 'success';
           this.timeoutId = setTimeout(() => {
             this.router.navigate(['/auth/login'])
               .catch(console.error)
               .finally(() => {
                 this.toastService.success('Account verified successfully!')
               })
           }, 3000)
         },
         error: (err) => {
           this.verificationStatus = 'failed';
           this.toastService.error('Failed to verify account');
         }
       })
   }

  ngOnDestroy(): void {
     if(this.timeoutId)
     {
       clearTimeout(this.timeoutId);
       console.trace('Cleared timeout')
     }
  }
}

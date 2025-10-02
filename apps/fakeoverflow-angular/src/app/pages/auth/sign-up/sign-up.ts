import {Component, OnInit, signal} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {Logo} from '@shared/logo/logo';
import {Password} from 'primeng/password';
import {Checkbox} from 'primeng/checkbox';
import {Button, ButtonDirective, ButtonIcon} from 'primeng/button';
import {Router, RouterLink} from '@angular/router';
import {InputGroup} from 'primeng/inputgroup';
import {Common} from '@services/common';
import {debounce, debounceTime, filter} from 'rxjs';
import {AuthService} from '../../../../../../../packages/fakeoverflow-angular-services/dist';
import {HotToastService} from '@ngxpert/hot-toast';
@Component({
  selector: 'app-sign-up',
  imports: [
    InputText,
    ReactiveFormsModule,
    Logo,
    Password,
    Checkbox,
    Button,
    RouterLink,
    InputGroup
  ],
  templateUrl: './sign-up.html',
  standalone: true,
  styleUrl: './sign-up.scss'
})
export class SignUp implements OnInit{
  signupForm: FormGroup;
  isLoading = signal(false);

  constructor(
    private fb: FormBuilder,
    private readonly common: Common,
    private readonly authService: AuthService,
    private readonly toastService : HotToastService,
    private readonly router: Router
    ) {
    this.signupForm = this.fb.group({
      displayName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
      acceptTerms: [false, [Validators.requiredTrue]],
      subscribeNewsletter: [false]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.displayName?.valueChanges
      .pipe(
        filter((v) => v?.toString().length > 3),
        debounceTime(1000)
      )
      .subscribe((x) => {
        this.searchForAvailability('displayName', x);
      })

    this.email?.valueChanges
      .pipe(
        filter((v) => v?.toString().length > 3),
        filter((v) => v?.toString().includes('@')), //Only send in email, I have designed the server to reject the request if the email is not valid
        debounceTime(1000)
      )
      .subscribe((x) => {
        this.searchForAvailability('email', x);
      })
  }

  private get displayName(){
    return this.signupForm.get('displayName');
  }

  private get email(){
    return this.signupForm.get('email');
  }

 passwordMatchValidator(form: AbstractControl): { [key: string]: any } | null {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    if (password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      const errors = confirmPassword.errors;
      if (errors) {
        delete errors['passwordMismatch'];
        confirmPassword.setErrors(Object.keys(errors).length ? errors : null);
      }
    }

    return null;
  }

  onSignup() {
    if(this.signupForm.invalid){
      console.trace('Form is invalid')
      return;
    }

    this.authService.signup({
      username: this.displayName?.value,
      password: this.signupForm.get('password')?.value,
      email: this.email?.value,
    })
      .pipe(
        this.toastService.observe({
          loading: 'Signing up...',
          error: 'Sign up failed!'
        })
      )
      .subscribe({
        next: (response) => {
          if(!response.userId)
          {
            this.toastService.error('Failed to signup');
            console.error('Failed to signup');
            return;
          }

          this.router.navigate(['/auth/verify'])
            .catch(console.error)
        },
        error: (err) => {
          console.error(err);
          if(err.status === 409){
          }
        }
      })
  }

  signupWithGoogle() {

  }

  generateRandomDisplayName() {
    const strings = this.common.generateRandomUserName();
    this.signupForm.patchValue({
      displayName: strings.join('-')
    });
  }

  searchForAvailability(type: 'email' | 'displayName', value: string) {
    this.authService.available({
      type: type === 'email' ? 'Email' : 'Username',
      value: value
    }).subscribe({
      next: (response) => {

      },
      error: (err) => {
        if(err.status === 409){
          this.signupForm.get(type)?.setErrors({
            'unique': "The value is not unique"
          })
          console.log(type)
          console.log(this.signupForm.get(type))
          console.log(this.signupForm.get(type)?.errors)
        }else {
          console.error(err);
        }
      }
    })
  }
}



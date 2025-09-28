import {Component, signal} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {Logo} from '@shared/logo/logo';
import {NgIf} from '@angular/common';
import {Password} from 'primeng/password';
import {Checkbox} from 'primeng/checkbox';
import {Button} from 'primeng/button';
import {RouterLink} from '@angular/router';
@Component({
  selector: 'app-sign-up',
  imports: [
    InputText,
    ReactiveFormsModule,
    Logo,
    NgIf,
    Password,
    Checkbox,
    Button,
    RouterLink
  ],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.scss'
})
export class SignUp {
  signupForm: FormGroup;
  isLoading = signal(false);

  constructor(private fb: FormBuilder) {
    this.signupForm = this.fb.group({
      displayName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
      acceptTerms: [false, [Validators.requiredTrue]],
      subscribeNewsletter: [false]
    }, { validators: this.passwordMatchValidator });
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
    if (this.signupForm.valid) {
      this.isLoading.set(true);

      // Get form values
      const formData = this.signupForm.value;

      // Remove confirmPassword from the data sent to API
      const { confirmPassword, ...signupData } = formData;

      // Simulate API call
      console.log('Signup attempt:', signupData);

      // Reset loading state after API call
      // You'll replace this with your actual API call
      setTimeout(() => {
        this.isLoading.set(false);
      }, 2000);
    } else {
      // Mark all fields as touched to show validation errors
      this.signupForm.markAllAsTouched();
    }
  }

  signupWithGoogle() {

  }
}



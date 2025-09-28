import {Component, inject, signal} from '@angular/core';
import {Navbar} from '@shared/navbar/navbar';
import {FormBuilder, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {Password} from 'primeng/password';
import {Checkbox} from 'primeng/checkbox';
import {Button} from 'primeng/button';
import {RouterLink} from '@angular/router';
import {Logo} from '@shared/logo/logo';
import {Authentication} from '@services/authentication';
import {AuthService, FakeoverFlowBackendHttpApiFeaturesAuthLoginLoginRequest} from 'fakeoverflow-angular-services';
import {HotToastService} from '@ngxpert/hot-toast';

@Component({
  selector: 'app-login',
  imports: [
    Navbar,
    FormsModule,
    InputText,
    ReactiveFormsModule,
    Password,
    Checkbox,
    Button,
    RouterLink,
    Logo
  ],
  templateUrl: './login.html',
  standalone: true,
  styleUrl: './login.scss'
})
export class Login {

  private readonly formBuilder = inject(FormBuilder);
  protected readonly loginForm = this.formBuilder.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(5)]],
  });
  protected rememberMe = false;
  protected isLoading = signal(false);
  private readonly authenticationService = inject(Authentication);
  private readonly authService = inject(AuthService);
  private readonly toastService: HotToastService = inject(HotToastService);

  loginWithGoogle() {

  }

  onLogin() {
    if(this.loginForm.invalid){
       this.toastService.error('Please fill all the fields')
      return;
    }

    const request : FakeoverFlowBackendHttpApiFeaturesAuthLoginLoginRequest = {
      type: 'Credentials',
      data: {}
    }

    const value = this.loginForm.value;
    request.data!['UserName'] = value.email!;
    request.data!['Password'] = value.password!;

    this.isLoading.set(true);
    this.authService.login(request)
      .pipe(
        this.toastService.observe({
          loading: 'Logging in...',
          success: 'Logged in successfully!',
          error: 'Login failed!'
        })
      )
      .subscribe({
        next: (res) => {
          const response = res.value;
          console.log(response);
        },
        error: (err) => {
          this.isLoading.set(false);
          console.error(err);
        }
      })
  }
}

import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ApiService } from '../../services/api';

@autoinject
export class Register {
  email = '';
  password = '';
  username = '';
  name = '';
  age: number | null = null;
  role = 'Client';

  error = '';
  success = '';

  constructor(
    private api: ApiService,
    private router: Router
  ) {}

  async register() {
    this.error = '';
    this.success = '';

    try {
      const result = await this.api.post('auth/register', {
        email: this.email,
        password: this.password,
        username: this.username,
        name: this.name,
        age: this.age ?? 0,
        role: this.role
      });

      if (result?.error) {
        this.error = result.error;
        return;
      }

      this.success = 'Registration successful. Please login.';
      setTimeout(() => {
        this.router.navigateToRoute('login');
      }, 1500);
    } catch (e) {
      console.error(e);
      this.error = 'Registration failed. Please check inputs.';
    }
  }
}

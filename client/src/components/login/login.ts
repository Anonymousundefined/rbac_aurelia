import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ApiService } from '../../services/api';

@autoinject
export class Login {
  email = '';
  password = '';
  error = '';

  constructor(
    private api: ApiService,
    private router: Router
  ) {}

  async login() {
    this.error = '';

    try {
      const result = await this.api.post('auth/login', {
        email: this.email,
        password: this.password
      });

     
      if (!result || !result.token) {
        this.error = 'Invalid login response';
        return;
      }
      localStorage.setItem('token', result.token);
      localStorage.setItem('user', JSON.stringify(result.user));

      this.api.setToken(result.token);

     
      this.router.navigateToRoute('dashboard');
    } catch (e) {
      console.error(e);
      this.error = 'Login failed. Check credentials or server.';
    }
  }
}

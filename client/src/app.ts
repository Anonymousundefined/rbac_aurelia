import { Router, RouterConfiguration } from 'aurelia-router';
import { PLATFORM } from 'aurelia-pal';

export class App {
  router: Router;

  configureRouter(config: RouterConfiguration, router: Router) {
    config.title = 'Aurelia RBAC';

    config.map([
      {
        route: ['', 'login'],
        name: 'login',
        moduleId: PLATFORM.moduleName('components/login/login'),
        nav: false,
        title: 'Login'
      },
      {
        route: 'register',
        name: 'register',
        moduleId: PLATFORM.moduleName('components/register/register'),
        nav: false,
        title: 'Register'
      },
      {
        route: 'dashboard',
        name: 'dashboard',
        moduleId: PLATFORM.moduleName('components/dashboard/dashboard'),
        nav: false,
        title: 'Dashboard'
      }
    ]);

    this.router = router;
  }
}

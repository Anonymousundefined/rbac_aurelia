import './styles/login.css';
import './styles/register.css';
import './styles/dashboard.css';
import { Aurelia } from 'aurelia-framework';
import { PLATFORM } from 'aurelia-pal';

export function configure(aurelia: Aurelia) {
  aurelia.use
    .standardConfiguration()
    .developmentLogging()
    .plugin(PLATFORM.moduleName('aurelia-router'));

  aurelia.start().then(() => {
    aurelia.setRoot(PLATFORM.moduleName('app'));
  });
}

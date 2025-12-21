
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ApiService } from '../../services/api';

@autoinject
export class Dashboard {
  user: any = null;

  policies: any[] = [];

  // Client submissions OR filtered admin submissions
  submissions: any[] = [];

  // Admin: full list (used for filtering)
  allSubmissions: any[] = [];


  policyContents: { [policyId: number]: string } = {};

  // Admin filter
  selectedStatus: string = 'All';

  constructor(
    private api: ApiService,
    private router: Router
  ) {}

  async attached() {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');

    if (!token || !userStr) {
      this.router.navigateToRoute('login');
      return;
    }

    this.user = JSON.parse(userStr);
    this.api.setToken(token);

    // Load policies (visible to both roles)
    this.policies = (await this.api.get('policies')) || [];

    // Client → only own submissions
    if (this.user.role === 'Client') {
      this.submissions = (await this.api.get('submissions')) || [];
    }

    // Admin → all submissions
    if (this.user.role === 'Admin') {
      this.allSubmissions = (await this.api.get('submissions')) || [];
      this.submissions = this.allSubmissions;
    }
  }

  // client

  async apply(policyId: number) {
    const content = this.policyContents[policyId];

    if (!content || !content.trim()) {
      return;
    }

    await this.api.post('submissions', {
      policyId,
      content
    });

    // clear only this policy's textarea
    this.policyContents[policyId] = '';

    // reload client submissions
    this.submissions = (await this.api.get('submissions')) || [];
  }

  // admin

  async approve(id: number) {
    await this.api.post(`submissions/${id}/review`, {
      status: 'Approved',
      comment: 'Approved by admin'
    });

    this.allSubmissions = (await this.api.get('submissions')) || [];
    this.applyFilter();
  }

  async reject(id: number) {
    await this.api.post(`submissions/${id}/review`, {
      status: 'Rejected',
      comment: 'Rejected by admin'
    });

    this.allSubmissions = (await this.api.get('submissions')) || [];
    this.applyFilter();
  }


  applyFilter() {
    if (this.selectedStatus === 'All') {
      this.submissions = this.allSubmissions;
    } else {
      this.submissions = this.allSubmissions.filter(
        s => s.status === this.selectedStatus
      );
    }
  }

  formatDate(date: string) {
    return date ? new Date(date).toLocaleDateString() : '';
  }

  logout() {
    localStorage.clear();
    this.router.navigateToRoute('login');
  }
}

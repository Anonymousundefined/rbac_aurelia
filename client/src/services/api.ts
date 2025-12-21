import { HttpClient } from 'aurelia-fetch-client';
import { autoinject } from 'aurelia-framework';

@autoinject
export class ApiService {
  private baseUrl = 'http://localhost:5000/api/';
  private token: string | null = null;

  constructor(private http: HttpClient) {
    http.configure(config => {
      config
        .withDefaults({
          headers: {
            'Content-Type': 'application/json'
          }
        });
    });
  }

  setToken(token: string) {
    this.token = token;
  }

  private async handleResponse(response: Response) {
    // 204 No Content OR empty body
    if (response.status === 204) {
      return null;
    }

    const text = await response.text();

    if (!text) {
      return null;
    }

    try {
      return JSON.parse(text);
    } catch {
      return null;
    }
  }

  async get(url: string) {
    const response = await this.http.fetch(this.baseUrl + url, {
      headers: this.authHeaders()
    });
    return this.handleResponse(response);
  }

  async post(url: string, body: any) {
    const response = await this.http.fetch(this.baseUrl + url, {
      method: 'POST',
      headers: this.authHeaders(),
      body: JSON.stringify(body)
    });
    return this.handleResponse(response);
  }

  async put(url: string, body?: any) {
    const response = await this.http.fetch(this.baseUrl + url, {
      method: 'PUT',
      headers: this.authHeaders(),
      body: body ? JSON.stringify(body) : null
    });
    return this.handleResponse(response);
  }

  private authHeaders() {
    return this.token
      ? { Authorization: `Bearer ${this.token}` }
      : {};
  }
}

export function getToken(): string | null {
  return localStorage.getItem('token');
}

export function getUser(): any | null {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
}

export function isLoggedIn(): boolean {
  return !!getToken();
}

export function isAdmin(): boolean {
  const user = getUser();
  return user && user.role === 'Admin';
}

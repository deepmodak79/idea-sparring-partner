import { TestBed } from '@angular/core/testing';
import { TokenStorage } from './token-storage.service';

describe('TokenStorage', () => {
  beforeEach(() => localStorage.clear());

  it('stores and clears tokens', () => {
    const storage = new TokenStorage();
    storage.saveTokens('access', 'refresh');
    expect(storage.getAccessToken()).toBe('access');
    expect(storage.getRefreshToken()).toBe('refresh');
    storage.clear();
    expect(storage.getAccessToken()).toBeNull();
  });
});

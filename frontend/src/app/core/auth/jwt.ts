import type { UserStatus } from '../models/enums';

// The role name seeded by the backend (RoleNames.Administrator).
export const ROLE_ADMIN = 'Administrator';

// The .NET default role claim URI, in case RoleClaimType wasn't remapped to "role".
const DOTNET_ROLE_CLAIM =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

export interface JwtClaims {
  sub?: string; // user id
  email?: string;
  name?: string; // "First Last"
  jti?: string;
  status?: UserStatus; // string claim
  exp?: number; // unix seconds
  // roles live under one of several keys — read via getRoles(), not directly
  [key: string]: unknown;
}

// Decode a JWT payload without verifying the signature (verification is the
// server's job; the client only reads claims for routing/UI).
export function decodeJwt(token: string): JwtClaims | null {
  const parts = token.split('.');
  if (parts.length !== 3) return null;
  try {
    const json = atob(base64UrlToBase64(parts[1]));
    // handle UTF-8 in claims (e.g. names) safely
    const decoded = decodeURIComponent(
      json
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );
    return JSON.parse(decoded) as JwtClaims;
  } catch {
    return null;
  }
}

export function isExpired(claims: JwtClaims | null): boolean {
  if (!claims?.exp) return true;
  // exp is seconds; Date.now() is ms
  return claims.exp * 1000 <= Date.now();
}

// Roles may arrive as "role", "roles", or the .NET URI, and as string | string[].
// Confirm the real key by decoding a live token once — then this stays defensive.
export function getRoles(claims: JwtClaims | null): string[] {
  if (!claims) return [];
  const raw =
    claims['role'] ?? claims['roles'] ?? claims[DOTNET_ROLE_CLAIM] ?? [];
  if (Array.isArray(raw)) return raw.map(String);
  if (typeof raw === 'string') return [raw];
  return [];
}

function base64UrlToBase64(input: string): string {
  let s = input.replace(/-/g, '+').replace(/_/g, '/');
  const pad = s.length % 4;
  if (pad) s += '='.repeat(4 - pad);
  return s;
}

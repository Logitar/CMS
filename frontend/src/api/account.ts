import type { CurrentUser, SaveProfilePayload, SignInPayload, UserProfile } from "@/types/account";
import { get, patch, post } from ".";

export async function getProfile(): Promise<UserProfile> {
  return (await get<UserProfile>("/api/profile")).data;
}

export async function saveProfile(payload: SaveProfilePayload): Promise<UserProfile> {
  return (await patch<SaveProfilePayload, UserProfile>("/api/profile", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  return (await post<SignInPayload, CurrentUser>("/api/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/api/sign/out");
}

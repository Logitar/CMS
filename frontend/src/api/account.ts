import type { CurrentUser, SaveProfilePayload, SignInPayload, UserProfile } from "@/types/account";
import { get, post } from ".";

export async function getProfile(): Promise<UserProfile> {
  return (await get<UserProfile>("/api/profile")).data;
}

export async function saveProfile(payload: SaveProfilePayload): Promise<UserProfile> {
  console.log(payload);
  throw new Error("NotImplemented");
}

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  return (await post<SignInPayload, CurrentUser>("/api/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/api/sign/out");
}

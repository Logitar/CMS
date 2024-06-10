import { urlUtils } from "logitar-js";

import type { CurrentUser, SaveProfilePayload, SignInPayload, UserProfile } from "@/types/account";
import { get, post } from ".";

export async function getProfile(): Promise<UserProfile> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/account/profile" }).buildRelative();
  return (await get<UserProfile>(url)).data;
}

export async function saveProfile(payload: SaveProfilePayload): Promise<UserProfile> {
  throw new Error("NotImplemented"); // TODO(fpion): implement
}

export async function signIn(payload: SignInPayload): Promise<CurrentUser> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/account/sign/in" }).buildRelative();
  return (await post<SignInPayload, CurrentUser>(url, payload)).data;
}

export async function signOut(): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/account/sign/out" }).buildRelative();
  await post(url);
}

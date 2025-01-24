import { defineStore } from "pinia";
import { ref } from "vue";

import type { Actor } from "@/types/actor";
import type { CurrentUser } from "@/types/account";

export const useAccountStore = defineStore(
  "account",
  () => {
    const currentUser = ref<CurrentUser>();

    function getActor(): Actor {
      return {
        id: "00000000-0000-0000-0000-000000000000",
        type: currentUser.value ? "User" : "System",
        isDeleted: false,
        displayName: currentUser.value?.displayName ?? "System",
        emailAddress: currentUser.value?.emailAddress,
        pictureUrl: currentUser.value?.pictureUrl,
      };
    }

    function signIn(user: CurrentUser): void {
      currentUser.value = user;
    }
    function signOut(): void {
      currentUser.value = undefined;
    }

    return { currentUser, getActor, signIn, signOut };
  },
  {
    persist: true,
  },
);

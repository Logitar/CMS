<script setup lang="ts">
import { useRouter } from "vue-router";

import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const router = useRouter();

router.push(account.currentUser ? { name: "ContentList" } : { name: "SignIn" });
</script>

<template>
  <main class="container"></main>
</template>

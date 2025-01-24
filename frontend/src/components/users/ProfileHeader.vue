<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { UserProfile } from "@/types/account";

const { d, t } = useI18n();

defineProps<{
  user: UserProfile;
}>();
</script>

<template>
  <table class="table table-striped">
    <tbody>
      <tr v-if="user.fullName">
        <th scope="row">{{ t("users.names.full") }}</th>
        <td>{{ user.fullName }}</td>
      </tr>
      <tr v-if="user.emailAddress">
        <th scope="row">{{ t("users.email.address") }}</th>
        <td>{{ user.emailAddress }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.createdOn") }}</th>
        <td>{{ d(user.createdOn, "medium") }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.updatedOn") }}</th>
        <td>{{ d(user.updatedOn, "medium") }}</td>
      </tr>
      <tr v-if="user.authenticatedOn">
        <th scope="row">{{ t("users.authenticatedOn") }}</th>
        <td>{{ d(user.authenticatedOn, "medium") }}</td>
      </tr>
    </tbody>
  </table>
</template>

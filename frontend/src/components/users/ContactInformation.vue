<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import EmailAddressInput from "./EmailAddressInput.vue";
import type { SaveProfilePayload, UserProfile } from "@/types/account";
import { saveProfile } from "@/api/account";

const props = defineProps<{
  user: UserProfile;
}>();

const emailAddress = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const user: UserProfile = props.user;
  return emailAddress.value !== (user.emailAddress ?? "");
});

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: UserProfile): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: SaveProfilePayload = {
      emailAddress: { value: emailAddress.value || undefined },
    };
    const user: UserProfile = await saveProfile(payload);
    emit("saved", user);
  } catch (e: unknown) {
    emit("error", e);
  }
});

watch(
  props.user,
  (user) => {
    emailAddress.value = user.emailAddress ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div class="mb-3">
      <AppSaveButton :disabled="isSubmitting || !hasChanges" exists :loading="isSubmitting" />
    </div>
    <EmailAddressInput v-model="emailAddress" />
  </form>
</template>

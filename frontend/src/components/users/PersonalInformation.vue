<script setup lang="ts">
import { computed, ref, watchEffect } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import PersonNameInput from "@/components/users/PersonNameInput.vue";
import PictureInput from "@/components/users/PictureInput.vue";
import type { SaveProfilePayload, UserProfile } from "@/types/account";
import { saveProfile } from "@/api/account";

const props = defineProps<{
  user: UserProfile;
}>();

const firstName = ref<string>("");
const lastName = ref<string>("");
const middleName = ref<string>("");
const pictureUrl = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const user: UserProfile = props.user;
  return (
    firstName.value !== (user.firstName ?? "") ||
    middleName.value !== (user.middleName ?? "") ||
    lastName.value !== (user.lastName ?? "") ||
    pictureUrl.value !== (user.pictureUrl ?? "")
  );
});

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: UserProfile): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: SaveProfilePayload = {
      firstName: { value: firstName.value },
      middleName: { value: middleName.value },
      lastName: { value: lastName.value },
      pictureUrl: { value: pictureUrl.value },
    };
    const user: UserProfile = await saveProfile(payload);
    emit("saved", user);
  } catch (e: unknown) {
    emit("error", e);
  }
});

watchEffect(() => {
  const user: UserProfile = props.user;
  firstName.value = user.firstName ?? "";
  lastName.value = user.lastName ?? "";
  middleName.value = user.middleName ?? "";
  pictureUrl.value = user.pictureUrl ?? "";
});
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div class="mb-3">
      <AppSaveButton :disabled="isSubmitting || !hasChanges" exists :loading="isSubmitting" />
    </div>
    <div class="row">
      <PersonNameInput class="col-lg-4" type="first" v-model="firstName" />
      <PersonNameInput class="col-lg-4" type="middle" v-model="middleName" />
      <PersonNameInput class="col-lg-4" type="last" v-model="lastName" />
    </div>
    <PictureInput v-model="pictureUrl" />
  </form>
</template>

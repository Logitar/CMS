<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import LocaleSelect from "@/components/users/LocaleSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Language } from "@/types/languages";
import { formatLanguage } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { readLanguage, setDefaultLanguage } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isDeleting = ref<boolean>(false);
const isSettingDefault = ref<boolean>(false);
const language = ref<Language>();

const formatted = computed<string>(() => (language.value ? formatLanguage(language.value) : ""));

function setModel(model: Language): void {
  language.value = model;
}

async function setDefault(): Promise<void> {
  if (language.value) {
    isSettingDefault.value = true;
    try {
      const updatedLanguage: Language = await setDefaultLanguage(language.value.id);
      setModel(updatedLanguage);
      toasts.success("languages.setDefault.success");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isSettingDefault.value = false;
    }
  }
}

function onDelete(hideModal: () => void): void {
  alert("Deleting languages is not implemented.");
  hideModal();
}

onMounted(async () => {
  try {
    const id: string = route.params.id.toString();
    const language = await readLanguage(id);
    setModel(language);
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="language">
      <h1>{{ formatted }}</h1>
      <StatusDetail :aggregate="language" />
      <div class="mb-3">
        <TarButton v-if="language.isDefault" class="me-1" disabled icon="fas fa-star" :text="t('languages.default')" variant="info" />
        <TarButton
          v-else
          class="me-1"
          icon="fas fa-star"
          :loading="isSettingDefault"
          :text="t('languages.setDefault.action')"
          variant="warning"
          @click="setDefault"
        />
        <AppBackButton class="mx-1" />
        <AppDelete
          class="ms-1"
          confirm="languages.delete.confirm"
          :displayName="formatted"
          :loading="isDeleting"
          title="languages.delete.title"
          @confirmed="onDelete"
        />
      </div>
      <LocaleSelect disabled label="languages.locale.label" :model-value="language.locale.code" placeholder="languages.locale.placeholder" />
    </template>
  </main>
</template>

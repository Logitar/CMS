<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentLocaleEdit from "@/components/contents/ContentLocaleEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Content, ContentLocale } from "@/types/contents";
import { formatLanguage } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readContent } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const content = ref<Content>();

// const locales = computed<ContentLocale[]>(() =>
//   content.value
//     ? orderBy(
//         content.value.locales.filter((locale) => Boolean(locale.language)),
//         "language.locale.displayName",
//       )
//     : [],
// ); // TODO(fpion): locales

function setModel(model: Content): void {
  content.value = model;
}

function onSaved(content: Content): void {
  setModel(content);
  toasts.success("contents.items.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const content: Content = await readContent(id);
      setModel(content);
    }
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
    <template v-if="content">
      <h1>{{ content.invariant.displayName ?? content.invariant.uniqueName }}</h1>
      <StatusDetail :aggregate="content" />
      <TarTabs>
        <TarTab active id="invariant" :title="t('contents.items.invariant')">
          <ContentLocaleEdit :content-id="content.id" :locale="content.invariant" @error="handleError" @saved="onSaved" />
        </TarTab>
        <!-- <TarTab v-for="locale in locales" :key="locale.language?.id" :id="locale.language?.id" :title="formatLanguage(locale.language)">
          <ContentLocaleEdit :content-id="content.id" :locale="locale" @error="handleError" @saved="onSaved" />
        </TarTab> -->
      </TarTabs>
    </template>
  </main>
</template>

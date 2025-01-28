<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentLocaleEdit from "@/components/contents/ContentLocaleEdit.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Actor } from "@/types/actor";
import type { ApiError } from "@/types/api";
import type { Content, ContentLocale } from "@/types/contents";
import type { Language } from "@/types/languages";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { readContent } from "@/api/contents";
import { useAccountStore } from "@/stores/account";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const content = ref<Content>();
const language = ref<Language>();
const newLocales = ref<Set<string>>(new Set<string>());

const isInvariant = computed<boolean>(() => Boolean(content.value && content.value.contentType.isInvariant));
const languageIds = computed<string[]>(() => (content.value?.locales ?? []).map((locale) => locale.language?.id ?? ""));
const locales = computed<ContentLocale[]>(() => (content.value ? content.value.locales.filter((locale) => Boolean(locale.language)) : []).sort(compare));

function addLocale(): void {
  if (content.value && language.value) {
    const actor: Actor = account.getActor();
    const now: string = new Date().toISOString();
    const locale: ContentLocale = {
      content: content.value,
      language: language.value,
      uniqueName: "",
      displayName: undefined,
      description: undefined,
      fieldValues: [],
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      isPublished: false,
      publishedBy: undefined,
      publishedOn: undefined,
    };
    content.value.locales.push(locale);
    newLocales.value.add(language.value.id);
    language.value = undefined;
  }
}

function compare(left: ContentLocale, right: ContentLocale): -1 | 0 | 1 {
  if (left.language && right.language) {
    if (left.language.locale.displayName < right.language.locale.displayName) {
      return -1;
    } else if (left.language.locale.displayName > right.language.locale.displayName) {
      return 1;
    }
  }
  return 0;
}

function isNewLocale(locale: ContentLocale): boolean {
  return Boolean(locale.language && newLocales.value.has(locale.language.id));
}

function onPublished(content: Content): void {
  setModel(content);
  toasts.success("contents.items.published");
}
function onUnpublished(content: Content): void {
  setModel(content);
  toasts.success("contents.items.unpublished");
}

function onSaved(content: Content, language?: Language): void {
  if (language) {
    newLocales.value.delete(language.id);
  }

  setModel(content);
  toasts.success("contents.items.updated");
}

function setModel(model: Content): void {
  content.value = model;
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
    if (status === StatusCodes.NotFound) {
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
      <StatusDetail v-if="!content.contentType.isInvariant" :aggregate="content" />
      <ContentLocaleEdit
        v-if="isInvariant"
        :content="content"
        :locale="content.invariant"
        @error="handleError"
        @published="onPublished"
        @saved="onSaved"
        @unpublished="onUnpublished"
      />
      <template v-else>
        <LanguageSelect :exclude="languageIds" no-status :model-value="language?.id" @selected="language = $event">
          <template #append>
            <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addLocale" />
          </template>
        </LanguageSelect>
        <TarTabs>
          <TarTab active id="invariant" :title="t('contents.items.invariant')">
            <ContentLocaleEdit
              :content="content"
              :locale="content.invariant"
              @error="handleError"
              @published="onPublished"
              @saved="onSaved"
              @unpublished="onUnpublished"
            />
          </TarTab>
          <TarTab v-for="locale in locales" :key="locale.language?.id" :id="locale.language?.id" :title="locale.language?.locale.displayName">
            <ContentLocaleEdit
              :content="content"
              :locale="locale"
              :new="isNewLocale(locale)"
              @error="handleError"
              @published="onPublished"
              @saved="onSaved($event, locale.language)"
              @unpublished="onUnpublished"
            />
          </TarTab>
        </TarTabs>
      </template>
    </template>
  </main>
</template>

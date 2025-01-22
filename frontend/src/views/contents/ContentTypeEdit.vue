<script setup lang="ts">
import { TarBadge, TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldDefinitionEdit from "@/components/fields/FieldDefinitionEdit.vue";
import InvariantCheckbox from "@/components/contents/InvariantCheckbox.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceContentTypePayload, ContentType } from "@/types/contents";
import type { FieldDefinition } from "@/types/fields";
import { formatFieldType } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readContentType, replaceContentType } from "@/api/contentTypes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const description = ref<string>("");
const displayName = ref<string>("");
const contentType = ref<ContentType>();
const isInvariant = ref<boolean>(false);
const uniqueName = ref<string>("");

const fields = computed<FieldDefinition[]>(() => (contentType.value ? orderBy(contentType.value.fields, "order") : []));
const hasChanges = computed<boolean>(() =>
  Boolean(
    contentType.value &&
      (isInvariant.value !== contentType.value.isInvariant ||
        uniqueName.value !== contentType.value.uniqueName ||
        displayName.value !== (contentType.value.displayName ?? "") ||
        description.value !== (contentType.value.description ?? "")),
  ),
);

function setModel(model: ContentType): void {
  contentType.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  isInvariant.value = model.isInvariant;
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (contentType.value) {
    try {
      const payload: CreateOrReplaceContentTypePayload = {
        isInvariant: isInvariant.value,
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
      };
      const updatedContentType: ContentType = await replaceContentType(contentType.value.id, payload, contentType.value.version);
      setModel(updatedContentType);
      toasts.success("contents.types.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

function onFieldDefinitionAdded(contentType: ContentType): void {
  setModel(contentType);
  toasts.success("fields.definitions.created");
}
function onFieldDefinitionUpdated(contentType: ContentType): void {
  setModel(contentType);
  toasts.success("fields.definitions.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const contentType: ContentType = await readContentType(id);
      setModel(contentType);
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
    <template v-if="contentType">
      <h1>{{ contentType.displayName ?? contentType.uniqueName }}</h1>
      <StatusDetail :aggregate="contentType" />
      <TarTabs>
        <TarTab active id="content-type" :title="t('contents.types.properties')">
          <form @submit.prevent="onSubmit">
            <InvariantCheckbox v-model="isInvariant" />
            <div class="row">
              <UniqueNameInput class="col" required v-model="uniqueName" />
              <DisplayNameInput class="col" v-model="displayName" />
            </div>
            <DescriptionTextarea v-model="description" />
            <div class="mb-3">
              <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
            </div>
          </form>
        </TarTab>
        <TarTab id="field-definitions" :title="t('fields.definitions.list')">
          <div class="my-3">
            <FieldDefinitionEdit :content-type-id="contentType.id" @error="handleError" @saved="onFieldDefinitionAdded" />
          </div>
          <table v-if="fields.length" class="table table-striped">
            <thead>
              <tr>
                <th scope="col">{{ t("uniqueName") }}</th>
                <th scope="col">{{ t("displayName") }}</th>
                <th scope="col">{{ t("fields.types.select.label") }}</th>
                <th scope="col"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="field in fields" :key="field.id">
                <td>
                  {{ field.uniqueName }}
                  <br />
                  <TarBadge v-if="field.isInvariant" class="me-1">{{ t("fields.definitions.invariant") }}</TarBadge>
                  <TarBadge v-if="field.isRequired" class="me-1">{{ t("fields.definitions.required") }}</TarBadge>
                  <TarBadge v-if="field.isIndexed" class="me-1">{{ t("fields.definitions.indexed") }}</TarBadge>
                  <TarBadge v-if="field.isUnique">{{ t("fields.definitions.unique") }}</TarBadge>
                </td>
                <td>{{ field.displayName ?? "—" }}</td>
                <td>
                  <RouterLink :to="{ name: 'FieldTypeEdit', params: { id: field.fieldType.id } }" target="_blank">
                    {{ formatFieldType(field.fieldType) }}
                  </RouterLink>
                </td>
                <td>
                  <FieldDefinitionEdit :content-type-id="contentType.id" :field="field" @error="handleError" @saved="onFieldDefinitionUpdated" />
                </td>
              </tr>
            </tbody>
          </table>
          <p v-else>{{ t("fields.definitions.empty") }}</p>
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>

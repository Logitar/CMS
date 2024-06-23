import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.MODE === "development" ? import.meta.env.BASE_URL : "/cms"),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      // route level code-splitting
      // this generates a separate chunk (ProfileView.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // Contents
    {
      name: "ContentItemList",
      path: "/contents",
      component: () => import("./views/contents/ContentItemList.vue"),
    },
    {
      name: "ContentItemEdit",
      path: "/contents/:id",
      component: () => import("./views/contents/ContentItemEdit.vue"),
    },
    {
      name: "ContentTypeList",
      path: "/content-types",
      component: () => import("./views/contents/ContentTypeList.vue"),
    },
    {
      name: "ContentTypeEdit",
      path: "/content-types/:id",
      component: () => import("./views/contents/ContentTypeEdit.vue"),
    },
    // Fields
    {
      name: "FieldTypeList",
      path: "/field-types",
      component: () => import("./views/fieldTypes/FieldTypeList.vue"),
    },
    {
      name: "FieldTypeEdit",
      path: "/field-types/:id",
      component: () => import("./views/fieldTypes/FieldTypeEdit.vue"),
    },
    // Languages
    {
      name: "LanguageList",
      path: "/languages",
      component: () => import("./views/languages/LanguageList.vue"),
    },
    {
      name: "LanguageEdit",
      path: "/languages/:id",
      component: () => import("./views/languages/LanguageEdit.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;

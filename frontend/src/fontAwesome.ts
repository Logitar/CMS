import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChevronLeft,
  faCog,
  faEdit,
  faHome,
  faKey,
  faListCheck,
  faPlus,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faTrashCan,
  faUser,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChevronLeft,
  faCog,
  faEdit,
  faHome,
  faKey,
  faListCheck,
  faPlus,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faTrashCan,
  faUser,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}

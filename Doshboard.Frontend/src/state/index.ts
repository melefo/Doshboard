import VuexPersistence from 'vuex-persist'
import Vuex from 'vuex'
import Vue from 'vue'
import { user } from '@/state/user'
import { widget } from '@/state/widget'
import { about } from '@/state/about'

Vue.use(Vuex)

const persistence = new VuexPersistence({
    storage: localStorage
})

export const store = new Vuex.Store({
    plugins: [persistence.plugin],
    modules: {
        user,
        widget,
        about
    }
})

export function authHeader() : HeadersInit {
    if (store.getters["user/isLoggedIn"])
      return { 'Authorization': 'Bearer ' + store.getters["user/token"] };
    return {};
  }
  
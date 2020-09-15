-------------------------------------------------------------------------
-- PKG_ACTOR_TYPES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_actor_types IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_actor_types (
        po_cursor   OUT sys_refcursor
    );

END pkg_actor_types;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_actor_types IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_actor_types (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                actor_type_id,
                actor_type_name
            from
                actor_types
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_actor_types' || chr(10) || sqlerrm);
    END p_get_all_actor_types;

END pkg_actor_types;
/
-------------------------------------------------------------------------